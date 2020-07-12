import { NowRequest, NowResponse } from "@vercel/node";
import { validateApiRequestInteractive } from "../services/api";

import fs from 'fs';

export default (req: NowRequest, res: NowResponse) => {
  validateApiRequestInteractive(req, res);
  fs.writeFileSync('./system-stats.json', req.body);
  res.send('OK');
}
