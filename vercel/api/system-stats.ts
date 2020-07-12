import { NowRequest, NowResponse } from "@vercel/node";
import fs from 'fs';

export default (req: NowRequest, res: NowResponse) => {
  const rawData = fs.readFileSync('./system-stats.json').toString();

  const stats = JSON.parse(rawData);
  res.json(stats);
}
