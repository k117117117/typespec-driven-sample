import { useRecordContext, useNotify, useRefresh } from "react-admin";
import { Button } from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import { useState } from "react";
import { apiClient } from "../api-client";

export const ApproveButton = () => {
    const record = useRecordContext();
    const notify = useNotify();
    const refresh = useRefresh();
    const [loading, setLoading] = useState(false);

    if (!record) return null;

    const handleApprove = async () => {
        setLoading(true);
        try {
            const { error } = await apiClient.POST(
                "/approval-requests/{id}/approve",
                { params: { path: { id: Number(record.id) } } }
            );
            if (error) throw error;
            notify("approve しました", { type: "success" });
            refresh();
        } catch {
            notify("approve に失敗しました", { type: "error" });
        } finally {
            setLoading(false);
        }
    };

    return (
        <Button
            variant="contained"
            color="success"
            startIcon={<CheckIcon />}
            onClick={handleApprove}
            disabled={loading}
        >
            Approve
        </Button>
    );
};
