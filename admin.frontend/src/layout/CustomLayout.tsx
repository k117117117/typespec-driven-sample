import { Layout } from 'react-admin';
import type { ReactNode } from 'react';
import { CustomMenu } from './CustomMenu';

export const CustomLayout = ({ children }: { children: ReactNode }) => (
    <Layout menu={CustomMenu}>
        {children}
    </Layout>
);
