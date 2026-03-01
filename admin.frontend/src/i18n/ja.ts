import japaneseMessages from "@bicstone/ra-language-japanese";
import { TranslationMessages } from "react-admin";

const ja: TranslationMessages = {
  ...japaneseMessages,
  resources: {
    "admin-tool-users": {
      name: "管理ツールユーザー",
      fields: {
        id: "ID",
        name: "名前",
        email: "メールアドレス",
        role: "ロール",
      },
    },
    "approval-requests": {
      name: "承認リクエスト",
      fields: {
        id: "ID",
        title: "タイトル",
        status: "ステータス",
        requestedBy: "申請者",
        reason: "理由",
      },
    },
  },
};

export default ja;
