import type { Metadata } from "next";
import { Cormorant } from "next/font/google";
import "./globals.css";

const cormorant = Cormorant({
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "PhotoDump",
  description: "Dump on your photos",
};

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
        {children}
      </body>
    </html>
  );
}
