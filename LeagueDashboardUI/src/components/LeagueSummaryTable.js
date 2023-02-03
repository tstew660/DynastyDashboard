import { Table } from 'react-bootstrap';
import { GetData } from './ApiAccess.js'
import { useLoaderData, useOutletContext} from 'react-router-dom';

export function LeagueSummaryTable() {
  const isSuperFlex = useOutletContext();
  const leagueObject = useLoaderData();
    
    let scoredRosters = AddPositionScores(leagueObject.rosters, isSuperFlex);
    isSuperFlex ? scoredRosters.sort((a, b) => (a.ktc_total_sf > b.ktc_total_sf) ? -1 : 1)
      : scoredRosters.sort((a, b) => (a.ktc_total_oneQB > b.ktc_total_oneQB) ? -1 : 1)
    
    return (
      <div>
        <div>
            {scoredRosters.length !== 0 ?
              <div>
                <Table striped bordered hover variant="light">
                  <thead>
                    <tr>
                      <th>Team Name</th>
                      <th>Team Dynasty Value</th>
                      <th>Team Redraft Value</th>
                      <th>QB Value</th>
                      <th>RB Value</th>
                      <th>WR Value</th>
                      <th>TE Value</th>
                    </tr>
                  </thead>
                  <tbody>
                    {scoredRosters && scoredRosters?.map((x) =>
                      <tr>
                        <td>{x.user.metadata.team_name}</td>
                        <td>{isSuperFlex ? x.ktc_total_sf : x.ktc_total_oneQB}</td>
                        <td>{isSuperFlex ? x.fp_total_sf : x.fp_total_oneQB}</td>
                        <td>{isSuperFlex ? x.qb_total_sf : x.qb_total_oneQB}</td>
                        <td>{isSuperFlex ? x.rb_total_sf : x.rb_total_oneQB}</td>
                        <td>{isSuperFlex ? x.wr_total_sf : x.wr_total_oneQB}</td>
                        <td>{isSuperFlex ? x.te_total_sf : x.te_total_oneQB}</td>
                      </tr>
                    )}
                  </tbody>
                </Table>
              </div> :
              <div>No league data to display</div>
          }
        </div>

      </div>
    );
  }

  

  const AddPositionScores = (rosters, superFlex) => {
    rosters.forEach((roster) => {
      let qbScore = 0;
      let rbScore = 0;
      let wrScore = 0;
      let teScore = 0;
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
      }
      else {
        roster.qb_total_oneQB = qbScore;
        roster.rb_total_oneQB = rbScore;
        roster.wr_total_oneQB = wrScore;
        roster.te_total_oneQB = teScore;
      }
      
    })
    return rosters;
  }
