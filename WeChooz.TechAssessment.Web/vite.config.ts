/**
 * Name: vite.config.ts
 * Description: Vite configuration file
 */
import react from "@vitejs/plugin-react";
import { spawn } from "child_process";
import fs from "fs";
import path from "path";
import * as process from "process";
import { defineConfig, type UserConfig } from "vite";
import viteTsconfigPaths from "vite-tsconfig-paths";

// Get base folder for certificates.
const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== "" ? `${process.env.APPDATA}/ASP.NET/https` : `${process.env.HOME}/.aspnet/https`;

// Generate the certificate name using the NPM package name
const certificateName = process.env.npm_package_name;

// Define certificate filepath
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
// Define key filepath
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);
// Pattern for CSS files
const cssPattern = /\.css$/;
// Pattern for image files
const imagePattern = /\.(png|jpe?g|gif|svg|webp|avif)$/;

const viteDevServerPort = 5180;

// Export Vite configuration
export default defineConfig(async () => {
    // Ensure the certificate and key exist
    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        if (!fs.existsSync(baseFolder)) {
            fs.mkdir(baseFolder, { recursive: true, mode: "0777" }, (err) => console.log(err));
        }
        // Wait for the certificate to be generated
        await new Promise<void>((resolve) => {
            spawn("dotnet", ["dev-certs", "https", "--export-path", certFilePath, "--format", "Pem", "--no-password"], { stdio: "inherit" }).on(
                "exit",
                (code: any) => {
                    resolve();
                    if (code) {
                        process.exit(code);
                    }
                },
            );
        });
    }

    const config: UserConfig = {
        plugins: [react(), viteTsconfigPaths({ loose: true })],
        clearScreen: true,
        appType: "custom",
        root: path.resolve(__dirname, "ClientApp", "src"),
        publicDir: path.resolve(__dirname, "ClientApp", "assets"),
        resolve: {
            alias: {
                "@mantine/core": path.resolve(__dirname, "node_modules/@mantine/core"),
                "@mantine/hooks": path.resolve(__dirname, "node_modules/@mantine/hooks"),
            },
        },
        define: {
            "global": "globalThis",
        },
        css: {
            preprocessorOptions: {
                scss: {
                    quietDeps: true,
                },
            },
            postcss: './postcss.config.js'
        },
        build: {
            manifest: "vite.manifest.json",
            emptyOutDir: true,
            outDir: path.resolve(__dirname, "wwwroot"),
            target: "esnext",
            assetsDir: "",
            rollupOptions: {
                input: {
                    index: path.resolve(__dirname, "ClientApp", "src", "public", "index.tsx"),
                    admin: path.resolve(__dirname, "ClientApp", "src", "admin", "index.tsx"),
                },
                output: {
                    // Save entry files to the appropriate folder
                    entryFileNames: "js/[name].[hash].js",
                    // Save chunk files to the js folder
                    chunkFileNames: "js/[name]-chunk.[hash].js",
                    // Save asset files to the appropriate folder
                    assetFileNames: (info) => {
                        if (info.name) {
                            // If the file is a CSS file, save it to the css folder
                            if (cssPattern.test(info.name)) {
                                return "css/[name].[hash][extname]";
                            }
                            // If the file is an image file, save it to the images folder
                            if (imagePattern.test(info.name)) {
                                return "images/[name].[hash][extname]";
                            }

                            // If the file is any other type of file, save it to the assets folder
                            return "assets/[name].[hash][extname]";
                        } else {
                            // If the file name is not specified, save it to the output directory
                            return "[name].[hash][extname]";
                        }
                    },
                },
            },
            commonjsOptions: {
                transformMixedEsModules: true,
            },
        },
        server: {
            origin: "http://localhost:" + viteDevServerPort,
            port: viteDevServerPort,
            strictPort: true,
            hmr: {
                port: viteDevServerPort,
            },

            /*     https: {
                     cert: certFilePath,
                     key: keyFilePath,
                 },*/
        },
    };

    return config;
});
