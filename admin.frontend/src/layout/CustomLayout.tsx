import { Layout } from 'react-admin';
import { CustomMenu } from './CustomMenu';

export const CustomLayout = ({ children }: { children: React.ReactNode }) => (
    <Layout menu={CustomMenu}>
        {children}
    </Layout>
);
