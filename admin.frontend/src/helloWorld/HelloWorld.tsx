import { Link } from "@mui/material";

export const HelloWorld = () => (
    <div>
        <h1>Hello, World!</h1>
        <p>CustomRoutesコンポーネントを使うと、独自のルートも追加することができます。</p>
        <Link href="https://marmelab.com/react-admin/CustomRoutes.html" target="_blank" rel="noopener">CustomRoutes component</Link>
        <p>カスタムルートは自動的にはMenuに追加されないので以下の方法で追加する必要がある</p>
        <Link href="https://marmelab.com/react-admin/CustomRoutes.html#adding-custom-routes-to-the-menu" target="_blank" rel="noopener">Adding Custom Routes to the Menu</Link>
    </div>
);
