import { Table, Container, Row, Col } from 'react-bootstrap';
import { GetData } from './ApiAccess.js'
import { useLoaderData, useOutletContext} from 'react-router-dom';
import { BestTeam } from './BestTeam.js';
import { WorstTeam } from './WorstTeam.js';
import { BestRedraftTeam } from './BestRedraftTeam.js';
import { WorstRedraftTeam } from './WorstRedraftTeam.js';
import { LeagueBarChart } from './LeagueBarChart.js';
import { SellTheFarm } from './SellTheFarm.js';
import { GoAllIn } from './GoAllIn.js';

export function LeagueSummaryTable() {
  const isSuperFlex = useOutletContext();
  const leagueObject = useLoaderData();
    
    let scoredRosters = AddPositionScores(leagueObject.rosters, isSuperFlex);
    isSuperFlex ? scoredRosters.sort((a, b) => (a.ktc_total_sf > b.ktc_total_sf) ? -1 : 1)
      : scoredRosters.sort((a, b) => (a.ktc_total_oneQB > b.ktc_total_oneQB) ? -1 : 1)
    
    return (
      <Container className='text-light'>
        <Row className="mb-2" fluid>
        <Container>
          <Col className="mb-2" >
            <BestTeam teams={leagueObject.rosters}></BestTeam>
          </Col>
          <Col className="mb-2">
            <WorstTeam teams={leagueObject.rosters}></WorstTeam>
          </Col>
          <Col className="mb-2">
            <BestRedraftTeam teams={leagueObject.rosters}></BestRedraftTeam>
          </Col>
          <Col className="mb-2">
            <WorstRedraftTeam teams={leagueObject.rosters}></WorstRedraftTeam>
          </Col>
          <Col className="mb-2">
            <SellTheFarm teams={leagueObject.rosters}></SellTheFarm>
          </Col>
          <Col className="mb-2">
            <GoAllIn teams={leagueObject.rosters}></GoAllIn>
          </Col>
          </Container>
        </Row>
        <Row>
          <Col className="mb-4">
          <LeagueBarChart teams={scoredRosters} />
          </Col>
        </Row>
        <Row className="mb-4">
          {scoredRosters.length !== 0 ?
            <Table striped bordered hover variant="dark" responsive className='table'>
              <thead>
                <tr>
                  <th>Team Name</th>
                  <th>Team Dynasty Value</th>
                  <th>Team Redraft Value</th>
                  <th>QB Value</th>
                  <th>RB Value</th>
                  <th>WR Value</th>
                  <th>TE Value</th>
                  <th>Draft Value</th>
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
                    <td>{isSuperFlex ? x.dp_total_sf : x.dp_total_oneQB}</td>
                  </tr>
                )}
              </tbody>
            </Table> :
            <div>No league data to display</div>
          }
        </Row>
      </Container>
    );
  }

  

  const AddPositionScores = (rosters, superFlex) => {
    rosters.forEach((roster) => {
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
      
    })
    return rosters;
  }
