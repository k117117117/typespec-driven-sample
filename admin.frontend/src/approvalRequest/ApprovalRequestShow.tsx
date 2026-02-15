import { ShowGuesser, FieldGuesser } from "@api-platform/admin";
import { Stack } from "@mui/material";
import { ApproveButton } from "./ApproveButton";
import { RejectButton } from "./RejectButton";

export const ApprovalRequestShow = () => (
    <ShowGuesser>
        <FieldGuesser source="id" />
        <FieldGuesser source="reason" />
        <FieldGuesser source="status" />
        <FieldGuesser source="createdAt" />
        <FieldGuesser source="updatedAt" />
        <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
            <ApproveButton />
            <RejectButton />
        </Stack>
    </ShowGuesser>
);
