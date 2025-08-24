import Link from "next/link";

export default function TopNav() {
    return (
        <div className="p-4">
            <h1 className="text-center text-7xl font-bold hover:cursor-pointer">
                <Link href={'/home'}>PhotoDump</Link>
            </h1> 
        </div>

    )
}