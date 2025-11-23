import Image from "next/image";

export default function Home() {
  return (
    <div className="h-full flex flex-col justify-center items-center">
      <h1 className="text-6xl text-white p-8">PhotoDump</h1>
      <Image 
        src={"/hello-just-kidding.jpeg"} 
        alt="hello. just kidding."
        width={400}
        height={400}
      />
    </div>
  );
}
