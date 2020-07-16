import { NowRequest, NowResponse } from "@vercel/node";
import { nowPlaying } from "../services/spotify";

export default async (req: NowRequest, res: NowResponse) => {
  const playing = await nowPlaying();
  res.json(playing);
}
