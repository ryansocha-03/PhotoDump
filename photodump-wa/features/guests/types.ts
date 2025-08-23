import z from "zod"

export type Guest = z.infer<typeof GuestSchema>;

export const GuestSchema = z.object({
    id: z.number(),
    name: z.string()
})