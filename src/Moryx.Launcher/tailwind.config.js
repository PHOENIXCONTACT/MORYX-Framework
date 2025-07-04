/** @type {import('tailwindcss').Config} */
const colors = require('tailwindcss/colors')
module.exports = {
    prefix: 'tw-',
    content: [
        "./Pages/**/*.{razor,html,cshtml}",
        "./components/src/**/*.{html,ts}"
    ],
    theme: {
        colors: {
            primary: {
                DEFAULT: '#007c84',
                50: '#e5f2f3',
                100: '#cce5e6',
                200: '#b3d8da',
                300: '#99cbce',
                400: '#80bdc1',
                500: '#66b0b5',
                600: '#4da3a9',
                700: '#33969d',
                800: '#1a8990',
                900: '#007c84',
            },
            success: {
                DEFAULT: '#96be0d',
                50: '#f4f8e7',
                100: '#eaf2cf',
                200: '#dfebb6',
                300: '#d5e59e',
                400: '#cbdf86',
                500: '#c0d86e',
                600: '#b6d156',
                700: '#abcb3d',
                800: '#a0c425',
                900: '#96be0d',
            },
            warning: {
                DEFAULT: '#e00a18',
                50: '#fce7e8',
                100: '#f9ced1',
                200: '#f6b6ba',
                300: '#f39da3',
                400: '#ef848b',
                500: '#ec6c74',
                600: '#e9545d',
                700: '#e63b46',
                800: '#e3232f',
                900: '#e00a18',
            },
            danger: '#e9545d',
            info: '#808080',
            fatal: '#800080',
            accent: '#abcb3d',
            background: '#fafafa',
            transparent: 'transparent',
            current: 'currentColor',
            black: colors.black,
            white: colors.white,
            gray: colors.gray,
            emerald: colors.emerald,
            indigo: colors.indigo,
            yellow: colors.yellow,
            red: colors.red,
        },
        zIndex: {
            '100': '100',
            '2': '2',
            '1': '1'
        },
        extend: {
            keyframes: {
                move: {
                    '0%': { top: '0px' },
                    '100%': { top: '100%' },
                }
            }
        },
    },
    plugins: [],
}