import Image from "next/image";

export default function NotFound() {
    return (
        <div className="h-full flex flex-col justify-center items-center">
            <h1 className="text-6xl pt-7">PhotoDump</h1>
            <h2 className="text-3xl p-6">404 | Not Found</h2>
            <Image
                src={"/hello-just-kidding.jpeg"} 
                alt="hello. just kidding."
                width={400}
                height={400}
            />
        </div>
    )
}