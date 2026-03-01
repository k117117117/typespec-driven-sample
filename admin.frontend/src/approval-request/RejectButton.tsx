import { useRecordContext, useNotify, useRefresh } from "react-admin";
import { Button } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { useState } from "react";
import { apiClient } from "../api-client";

export const RejectButton = () => {
    const record = useRecordContext();
    const notify = useNotify();
    const refresh = useRefresh();
    const [loading, setLoading] = useState(false);

    if (!record) return null;

    const handleReject = async () => {
        setLoading(true);
        try {
            const { error } = await apiClient.POST(
                "/approval-requests/{id}/reject",
                { params: { path: { id: Number(record.id) } } }
            );
            if (error) throw error;
            notify("reject しました", { type: "success" });
            refresh();
        } catch {
            notify("reject に失敗しました", { type: "error" });
        } finally {
            setLoading(false);
        }
    };

    return (
        <Button
            variant="contained"
            color="error"
            startIcon={<CloseIcon />}
            onClick={handleReject}
            disabled={loading}
        >
            Reject
        </Button>
    );
};
