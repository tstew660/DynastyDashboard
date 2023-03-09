import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    Title,
    Tooltip,
    Legend,
    ArcElement,
  } from 'chart.js';
  import { Pie } from 'react-chartjs-2';
  import { useOutletContext } from 'react-router-dom';
  import { AddPositionScores } from './AddPositionScores';

ChartJS.register(
  CategoryScale,
  ArcElement,
  LinearScale,
  Title,
  Tooltip,
  Legend
);



export function TeamPieChart({team}) {
    const isSuperFlex = useOutletContext();
    const options = {
        plugins: {
          title: {
            display: true,
            text: "Team Position Breakdown",
          },
        },
        maintainAspectRatio : false,
        scales: {
          x: {
            stacked: true,
            display: false
          },
          y: {
            stacked: true,
            display: false
          },
        },
      };
      let teamScored = AddPositionScores(team, isSuperFlex);
      console.log(team);
      const data = {
          labels: ['QB', 'RB', 'WR', 'TE', 'Draft Cap'],
          datasets: [
            {
              label: 'Score',
              data: [isSuperFlex ? teamScored.qb_total_sf : teamScored.qb_total_oneQB,
                isSuperFlex ? teamScored.rb_total_sf : teamScored.rb_total_oneQB,
                isSuperFlex ? teamScored.wr_total_sf : teamScored.wr_total_oneQB,
                isSuperFlex ? teamScored.te_total_sf : teamScored.te_total_oneQB,
                isSuperFlex ? teamScored.dp_total_sf : teamScored.dp_total_oneQB],
              backgroundColor: ['red', 'blue', 'yellow', 'green', 'orange'],
            }
          ],
        };
      
  return (
    <article className="canvas-container">
  <Pie options={options} data={data} />
  </article>);
}