import fetch from 'isomorphic-unfetch';
import { SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET, SPOTIFY_REFRESH_TOKEN } from "./env";
import { stringify } from 'querystring';

const TOKEN_URL = 'https://accounts.spotify.com/api/token';
const NOW_PLAYING_ENDPOINT = `https://api.spotify.com/v1/me/player/currently-playing`;

const basicToken = Buffer.from(`${SPOTIFY_CLIENT_ID}:${SPOTIFY_CLIENT_SECRET}`).toString('base64');

async function getAccessToken() {
  let body = {
    grant_type: 'refresh_token',
    SPOTIFY_REFRESH_TOKEN
  };

  const response = await fetch(TOKEN_URL, {
    method: 'POST',
    headers: {
      'Authorization': `Basic ${basicToken}`,
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    body: stringify(body)
  }).then(r => r.json());

  return response.access_token;
}

export async function nowPlaying() {
  const token = getAccessToken();
  const response = await fetch(NOW_PLAYING_ENDPOINT, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  if (!response.ok)
    throw 'Error while fetching now playing from Spotify';
  
  const data = await response.json();
  return data;
}
