import { createSlice } from "@reduxjs/toolkit";


export const leagueSlice = createSlice({
    name: "leagues",
    initialState: {value: {leagueId: "", teamId: "1"} },
    reducers: {
        updateLeague: (state, action) => {
            state.value.leagueId = action.payload;
        },
        updateTeam: (state, action) => {
            state.value.teamId = action.payload;
        }
    }
})

export const {updateLeague, updateTeam} = leagueSlice.actions;
export default leagueSlice.reducer;