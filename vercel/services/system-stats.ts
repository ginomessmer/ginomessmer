import Axios from "axios";
import * as config from '../config';

export const getSystemStats = () => {
  return Axios.get(config.SYSTEM_STATS_ENDPOINT);
}