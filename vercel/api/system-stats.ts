import { NowRequest, NowResponse } from "@vercel/node";
import { getSystemStats } from "../services/system-stats";

export default async (req: NowRequest, res: NowResponse) => {
  const stats = await getSystemStats();
  res.json(stats.data);
}
