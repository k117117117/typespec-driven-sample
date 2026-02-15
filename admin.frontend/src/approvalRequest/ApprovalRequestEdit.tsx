import { EditGuesser, InputGuesser } from "@api-platform/admin";

export const ApprovalRequestEdit = () => (
    <EditGuesser>
        <InputGuesser source="id" readOnly />
        <InputGuesser source="reason" />
        <InputGuesser source="status" />
        <InputGuesser source="createdAt" readOnly />
        <InputGuesser source="updatedAt" readOnly />
    </EditGuesser>
);
