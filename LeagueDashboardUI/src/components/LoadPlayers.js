import { GetData } from "./ApiAccess";
import { ApiEndpoint } from "../ApiConfig";

export const LoadPlayers = async ({ request }) => {

    const endpoint = ApiEndpoint();
    const res = await GetData(`${endpoint}/players/GetAllPlayersAsync?pageNumber=1&pageSize=25`);
    return res;
  }