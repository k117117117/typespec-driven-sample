import { Menu } from 'react-admin';
import { routes } from '../routes';

export const CustomMenu = () => (
    <Menu>
        <Menu.ResourceItems />
        <Menu.Item to={routes.helloWorld} primaryText="Hello World" leftIcon={<>👋</>} />
    </Menu>
);
