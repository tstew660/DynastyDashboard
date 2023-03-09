export const AddPositionScores = (roster, superFlex) => {
      let qbScore = 0;
      let rbScore = 0;
      let wrScore = 0;
      let teScore = 0;
      let dPScore = 0;
      roster.picks.forEach((pick) => {
        if(superFlex)
          {
            if(pick.rank_sf != null) {
              dPScore += pick.rank_sf;
            }  
          }
          else {
            if(pick.rank_oneQB != null) {
              dPScore += pick.rank_oneQB;
            } 
          }   
      })
      roster.playersList.forEach((player) => {
        if(player.position === "qb") {
          if(superFlex)
          {
            if(player.ktc_rank_sf != null) {
              qbScore += player.ktc_rank_sf;
            }  
          }
          else {
            if(player.ktc_rank_oneQB != null) {
              qbScore += player.ktc_rank_oneQB;
            } 
          }   
        }
        if(player.position === "rb") {
          if(superFlex)
          {
            if(player.ktc_rank_sf != null) {
              rbScore += player.ktc_rank_sf;
            }  
          }
          else {
            if(player.ktc_rank_oneQB != null) {
              rbScore += player.ktc_rank_oneQB;
            } 
          }   
        }
        if(player.position === "wr") {
          if(superFlex)
          {
            if(player.ktc_rank_sf != null) {
              wrScore += player.ktc_rank_sf;
            }  
          }
          else {
            if(player.ktc_rank_oneQB != null) {
              wrScore += player.ktc_rank_oneQB;
            } 
          }   
        }
        if(player.position === "te") {
          if(superFlex)
          {
            if(player.ktc_rank_sf != null) {
              teScore += player.ktc_rank_sf;
            }  
          }
          else {
            if(player.ktc_rank_oneQB != null) {
              teScore += player.ktc_rank_oneQB;
            } 
          }   
        }
      })
      if(superFlex){
        roster.qb_total_sf = qbScore;
        roster.rb_total_sf = rbScore;
        roster.wr_total_sf = wrScore;
        roster.te_total_sf = teScore;
        roster.dp_total_sf = dPScore;
      }
      else {
        roster.qb_total_oneQB = qbScore;
        roster.rb_total_oneQB = rbScore;
        roster.wr_total_oneQB = wrScore;
        roster.te_total_oneQB = teScore;
        roster.dp_total_oneQB = dPScore;
      }
    return roster;
  }