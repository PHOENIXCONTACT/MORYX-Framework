declare module 'react-bootstrap-toggle' {

    export interface BootstrapToggleProps {
        onstyle?: string;
        onClassName?: string;
        offstyle?: string;
        offClassName?: string;
        handlestyle?: string;
        handleClassName?: string;
        height?: number | string;
        width?: number | string;
        on?: React.ReactNode;
        off?: React.ReactNode;
        active?: boolean;
        disabled?: boolean;
        onClick: (e: React.MouseEvent<HTMLElement>) => void;
    }
    
    const BootstrapToggle: React.StatelessComponent<BootstrapToggleProps>;
    export default BootstrapToggle;
}
