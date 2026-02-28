#!/usr/bin/env node
/**
 * TypeSpec → OpenAPI のスキーマに存在しない孤立した *Entity.cs ファイルを検出・削除するスクリプト。
 *
 * tsp compile → NSwag 生成 の後に実行することで、TypeSpec 側で削除されたモデルに
 * 対応する C# エンティティファイルを自動的にクリーンアップする。
 *
 * 判定ロジック:
 *   「OpenAPI スキーマにも AppDbContext の DbSet にも対応がない *Entity.cs」を孤立とみなす。
 *   → DB 専用エンティティ（API に露出しないが DbSet に登録されているもの）は削除されない。
 *
 * Usage: node scripts/clean-orphan-entities.mjs
 */
import { readFileSync, readdirSync, unlinkSync, existsSync } from "fs";
import { join } from "path";

/** 各バックエンドの OpenAPI JSON パス、エンティティ格納ディレクトリ、DbContext パスの対応 */
const configs = [
  {
    name: "admin.backend",
    openapi: "typespec/tsp-output/schema/openapi.admin.json",
    entitiesDir: "admin.backend/Data",
    dbContext: "admin.backend/Data/AppDbContext.cs",
  },
  {
    name: "game.server",
    openapi: "typespec/tsp-output/schema/openapi.gameserver.json",
    entitiesDir: "game.server/Data",
    dbContext: "game.server/Data/AppDbContext.cs",
  },
];

/**
 * AppDbContext.cs から DbSet<T> に登録されているエンティティ名を抽出する。
 * 例: DbSet<PlayerEntity> → "Player"
 */
function extractDbSetEntities(dbContextPath) {
  const names = new Set();
  if (!existsSync(dbContextPath)) return names;
  const content = readFileSync(dbContextPath, "utf-8");
  const re = /DbSet<(\w+?)Entity>/g;
  let match;
  while ((match = re.exec(content)) !== null) {
    names.add(match[1]);
  }
  return names;
}

let removedCount = 0;

for (const { name, openapi, entitiesDir, dbContext } of configs) {
  if (!existsSync(openapi)) {
    console.log(`⏭  ${name}: ${openapi} が見つかりません、スキップ`);
    continue;
  }
  if (!existsSync(entitiesDir)) {
    // Data/ ディレクトリがなければスキップ（まだ実装されていないサービス等）
    continue;
  }

  const spec = JSON.parse(readFileSync(openapi, "utf-8"));
  const schemas = Object.keys(spec.components?.schemas ?? {});

  // スキーマ名からベースモデル名を抽出
  // 例: ReadAdminToolUser, CreateAdminToolUser, ReadAdminToolUserItem → AdminToolUser
  const modelNames = new Set();
  for (const s of schemas) {
    const m = s.match(/^(?:Read|Create|Update)(.+?)(?:Item)?$/);
    if (m) {
      modelNames.add(m[1]);
    } else {
      modelNames.add(s);
    }
  }

  // AppDbContext の DbSet に登録されているエンティティは削除対象外
  const dbSetEntities = extractDbSetEntities(dbContext);

  // *Entity.cs ファイルをスキャンし、OpenAPI にも DbSet にも存在しなければ削除
  const files = readdirSync(entitiesDir).filter((f) => f.endsWith("Entity.cs"));
  for (const file of files) {
    const baseName = file.replace(/Entity\.cs$/, "");
    if (!modelNames.has(baseName) && !dbSetEntities.has(baseName)) {
      const fullPath = join(entitiesDir, file);
      console.log(
        `🗑  ${name}: 孤立エンティティを削除 → ${file} (スキーマ "${baseName}" が OpenAPI にも DbSet にも存在しません)`
      );
      unlinkSync(fullPath);
      removedCount++;
    }
  }
}

if (removedCount === 0) {
  console.log("✅ 孤立エンティティなし");
} else {
  console.log(`✅ ${removedCount} 件の孤立エンティティを削除しました`);
}
