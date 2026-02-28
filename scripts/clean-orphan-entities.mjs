#!/usr/bin/env node
/**
 * TypeSpec → OpenAPI のスキーマに存在しない孤立した *Entity.cs ファイルを検出・削除するスクリプト。
 *
 * tsp compile → NSwag 生成 の後に実行することで、TypeSpec 側で削除されたモデルに
 * 対応する C# エンティティファイルを自動的にクリーンアップする。
 *
 * Usage: node scripts/clean-orphan-entities.mjs
 */
import { readFileSync, readdirSync, unlinkSync, existsSync } from "fs";
import { join } from "path";

/** 各バックエンドの OpenAPI JSON パスとエンティティ格納ディレクトリの対応 */
const configs = [
  {
    name: "admin.backend",
    openapi: "typespec/tsp-output/schema/openapi.admin.json",
    entitiesDir: "admin.backend/Data",
  },
  {
    name: "game.server",
    openapi: "typespec/tsp-output/schema/openapi.gameserver.json",
    entitiesDir: "game.server/Data",
  },
];

let removedCount = 0;

for (const { name, openapi, entitiesDir } of configs) {
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

  // *Entity.cs ファイルをスキャンし、対応するモデルが OpenAPI に存在しなければ削除
  const files = readdirSync(entitiesDir).filter((f) => f.endsWith("Entity.cs"));
  for (const file of files) {
    const baseName = file.replace(/Entity\.cs$/, "");
    if (!modelNames.has(baseName)) {
      const fullPath = join(entitiesDir, file);
      console.log(
        `🗑  ${name}: 孤立エンティティを削除 → ${file} (スキーマ "${baseName}" が OpenAPI に存在しません)`
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
