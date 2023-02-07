import { GetData } from "./ApiAccess";
import { ApiEndpoint } from "../ApiConfig";

export const LoadRosters = async ({ params }) => {
    const endpoint = ApiEndpoint();
    const res = await GetData(`${endpoint}/League/${params.leagueId}`);
    return res;
  }