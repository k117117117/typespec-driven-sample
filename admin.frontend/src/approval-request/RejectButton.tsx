import { useRecordContext, useNotify, useRefresh } from "react-admin";
import { Button } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { API_URL } from "../App";

export const RejectButton = () => {
    const record = useRecordContext();
    const notify = useNotify();
    const refresh = useRefresh();

    if (!record) return null;

    const handleReject = async () => {
        try {
            // RPCエンドポイントはfetchで直接呼び出すか、Typespecから生成したAPIクライアントを使用して呼び出すなどが良いのかもしれない
            // https://marmelab.com/react-admin/Actions.html#querying-the-api-with-fetch
            const res = await fetch(
                `${API_URL}/approval-requests/${record.id}/reject`,
                { method: "POST" }
            );
            if (!res.ok) throw new Error("reject failed");
            
            notify("reject しました", { type: "success" });
            refresh();
        } catch {
            notify("reject に失敗しました", { type: "error" });
        }
    };

    return (
        <Button
            variant="contained"
            color="error"
            startIcon={<CloseIcon />}
            onClick={handleReject}
        >
            Reject
        </Button>
    );
};
