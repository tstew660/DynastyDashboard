import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
  } from 'chart.js';
  import { Bar } from 'react-chartjs-2';
  import { useOutletContext } from 'react-router-dom';

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

  

export function LeagueBarChart({teams}) {
    const isSuperFlex = useOutletContext();
    const options = {
        plugins: {
          title: {
            display: true,
            text: teams.name + " - League Totals",
          },
        },
        responsive: true,
        scales: {
          x: {
            stacked: true,
          },
          y: {
            stacked: true,
          },
        },
      };
      
      const labels = teams.rosters.map(x => {
          return x.user.metadata.team_name;
      })
      const data = {
          labels,
          datasets: [
            {
              label: 'QB',
              data: isSuperFlex ? teams.rosters.map((team) => team.qb_total_sf): teams.rosters.map((team) => team.qb_total_oneQB),
              backgroundColor: 'red',
            },
            {
              label: 'RB',
              data: isSuperFlex ? teams.rosters.map((team) => team.rb_total_sf): teams.rosters.map((team) => team.rb_total_oneQB),
              backgroundColor: 'blue',
            },
            {
              label: 'WR',
              data: isSuperFlex ? teams.rosters.map((team) => team.wr_total_sf): teams.rosters.map((team) => team.wr_total_oneQB),
              backgroundColor: 'yellow',
            },
            {
              label: 'TE',
              data: isSuperFlex ? teams.rosters.map((team) => team.te_total_sf): teams.rosters.map((team) => team.te_total_oneQB),
              backgroundColor: 'green',
            }
          ],
        };
      
  return <Bar options={options} data={data} />;
}