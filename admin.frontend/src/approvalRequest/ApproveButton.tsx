import { useRecordContext, useNotify, useRefresh } from "react-admin";
import { Button } from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import { API_URL } from "../App";

export const ApproveButton = () => {
    const record = useRecordContext();
    const notify = useNotify();
    const refresh = useRefresh();

    if (!record) return null;

    const handleApprove = async () => {
        try {
            // RPCエンドポイントはfetchで直接呼び出すか、Typespecから生成したAPIクライアントを使用して呼び出すなどが良いのかもしれない
            // https://marmelab.com/react-admin/Actions.html#querying-the-api-with-fetch
            const res = await fetch(
                `${API_URL}/approval-requests/${record.id}/approve`,
                { method: "POST" }
            );
            if (!res.ok) throw new Error("approve failed");
            
            notify("approve しました", { type: "success" });
            refresh();
        } catch {
            notify("approve に失敗しました", { type: "error" });
        }
    };

    return (
        <Button
            variant="contained"
            color="success"
            startIcon={<CheckIcon />}
            onClick={handleApprove}
        >
            Approve
        </Button>
    );
};
