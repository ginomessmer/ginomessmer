import { NowRequest, NowResponse } from "@vercel/node";
import { renderToString } from 'react-dom/server';

import { getSystemStats } from "../services/system-stats";
import CpuCard from "../components/CpuCard";

export default async (req: NowRequest, res: NowResponse) => {
  const response = await getSystemStats();
  
  res.setHeader("Content-Type", "image/svg+xml");
  res.setHeader("Cache-Control", "no-cache");

  const card = renderToString(CpuCard({

  }));

  return res.status(200).send(card);
}
