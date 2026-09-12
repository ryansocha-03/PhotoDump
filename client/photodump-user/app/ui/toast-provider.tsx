'use client'

import { Toaster } from "react-hot-toast";

export default function Providers({ children}: { children: React.ReactNode }) {
    return (
        <>
            {children}
            <Toaster 
                position="top-right" 
                reverseOrder={false} 
                toastOptions={{
                    style: {
                        background: 'var(--background)',
                        color: 'var(--foreground)',
                    },
                    success: {
                        duration: 5000,
                        style: {
                            background: 'var(--background)',
                            color: 'var(--foreground)',
                            border: '1px solid var(--foreground)',
                        },
                    },
                    error: {
                        duration: 5000,
                        style: {
                            background: 'var(--background)',
                            color: 'var(--foreground)',
                            border: '1px solid var(--foreground)',
                        },
                    },
                }}
            />
        </>
    )
}