import { getEventLandingData } from "@/app/lib/event/data";
import { notFound } from "next/navigation";

export default async function Layout({
  children,
  params
}: {
  children: React.ReactNode,
  params: Promise<{ publicEventId: string }>
}){
  const { publicEventId } = await params;
  const eventData = await getEventLandingData(publicEventId);

  if (eventData.code == 404) {
      notFound()
  }
  else if (eventData.code != 200) {
      throw new Error("BRUH")
  }

  return (
    <div>
        <div className="text-4xl m-8 text-center">{eventData.data?.eventName}</div>
        {children}
    </div>
  );
}