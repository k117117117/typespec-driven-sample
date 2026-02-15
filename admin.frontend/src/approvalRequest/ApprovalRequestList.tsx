import { ListGuesser, FieldGuesser } from "@api-platform/admin";

export const ApprovalRequestList = () => (
    <ListGuesser>
        <FieldGuesser source="id" />
        <FieldGuesser source="reason" />
        <FieldGuesser source="status" />
        <FieldGuesser source="createdAt" />
        <FieldGuesser source="updatedAt" />
    </ListGuesser>
);
