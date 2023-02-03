import { GetData } from "./ApiAccess";

export const LoadRosters = async ({ params }) => {
    console.log(params.leagueId);
    const res = await GetData(`https://localhost:44380/api/League/${params.leagueId}`);
    return res;
  }