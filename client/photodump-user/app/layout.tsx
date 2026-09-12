import type { Metadata } from "next";
import { Cormorant } from "next/font/google";
import "./globals.css";
import Providers from "./ui/toast-provider";

const cormorant = Cormorant({
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "PhotoDump",
  description: "Dump on your photos",
};

/**
 * The root layout component for the application. It wraps the entire application with HTML and body tags, applying global styles and fonts.
 */
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${cormorant.className} m-8 antialiased`}
      >
        <Providers>
          {children}
        </Providers>
      </body>
    </html>
  );
}
