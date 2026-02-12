/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./ClientApp/src/**/*.{js,ts,jsx,tsx}",
        "./Admin/_Views/**/*.cshtml",
        // Ajoute d'autres dossiers de vues Razor si besoin :
        "./Home/_Views/**/*.cshtml"
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}