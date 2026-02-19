
import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { MantineProvider } from "@mantine/core";
import '@mantine/core/styles.layer.css';
import './index.css';
import PublicCatalogPage from "./pages/PublicCatalogPage";

const root = ReactDOM.createRoot(document.getElementById("react-app") as HTMLElement);
root.render(
    <React.StrictMode>
        <MantineProvider defaultColorScheme="light">
            <BrowserRouter>
                <Routes>
                    <Route path="*" element={<PublicCatalogPage />} />
                </Routes>
            </BrowserRouter>
        </MantineProvider>
    </React.StrictMode>
);
