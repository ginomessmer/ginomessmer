import { API_SECRET } from "./env";
import { NowRequest, NowResponse } from "@vercel/node";

export const validateApiRequest = (req: NowRequest) => req.headers.authorization === `Bearer ${API_SECRET}`;
export const validateApiRequestInteractive = (req: NowRequest, res: NowResponse) => {
  if (!validateApiRequest(req)) {
    res.statusCode = 401;
    res.send('Unauthorized');
  }
}
