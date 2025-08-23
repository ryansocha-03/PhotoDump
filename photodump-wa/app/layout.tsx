import type { Metadata } from "next";
import { Roboto, Roboto_Mono } from "next/font/google";
import "@/styles/globals.css";
import TopNav from "@/ui/top-nav";

const roboto = Roboto({
  subsets: ["latin"],
});

const robotoMono = Roboto_Mono({
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "PhotoDump",
  description: "Put yer photos here",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={`${roboto.className} antialiased`}>
      <body> 
        <TopNav /> 
        {children}
      </body>
    </html>
  );
}
