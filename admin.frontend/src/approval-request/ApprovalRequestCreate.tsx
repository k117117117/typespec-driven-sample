import { CreateGuesser, InputGuesser } from "@api-platform/admin";

export const ApprovalRequestCreate = () => (
    <CreateGuesser>
        <InputGuesser source="reason" />
    </CreateGuesser>
);
