
import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import './index.css';
import PublicCatalogPage from "./pages/PublicCatalogPage";
const root = ReactDOM.createRoot(document.getElementById("react-app") as HTMLElement);
root.render(
    <React.StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="*" element={<PublicCatalogPage />} />
            </Routes>
        </BrowserRouter>
    </React.StrictMode>
);
