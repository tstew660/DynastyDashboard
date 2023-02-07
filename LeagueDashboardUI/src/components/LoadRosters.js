import { GetData } from "./ApiAccess";
import { ApiEndpoint } from "../ApiConfig";

export const LoadRosters = async ({ params }) => {
    console.log(params.leagueId);
    const res = await GetData(`${ApiEndpoint}/League/${params.leagueId}`);
    return res;
  }