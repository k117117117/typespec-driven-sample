import { Menu } from 'react-admin';

export const CustomMenu = () => (
    <Menu>
        <Menu.ResourceItems />
        <Menu.Item to="/hello-world" primaryText="Hello World" leftIcon={<>👋</>} />
    </Menu>
);
