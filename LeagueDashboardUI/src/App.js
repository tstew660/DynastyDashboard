import './App.css';
import { useState } from 'react'

import "bootstrap/dist/css/bootstrap.min.css";
import { LeagueSummaryTable, DataLoader } from './components/LeagueSummaryTable';
import { MainNavigation } from './components/SidebarNav';
import { UserNameform } from './components/UserNameForm';
import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom"
import TeamOverviewComponent from './components/TeamOverviewComponent';
import { LoadRosters } from './components/LoadRosters';

export default function App() {

  const router = createBrowserRouter([
    {
      path: "/",
      element: < MainNavigation />,
      children: [
        {
          path: "/",
          element: <UserNameform />
        },
        {
          path: "/overview/:leagueId",
          loader: LoadRosters,
          element: <LeagueSummaryTable  />  
        },
        {
          path: "/team/:leagueId/:teamId",
          loader: LoadRosters,
          element: <TeamOverviewComponent />
        }
      ]
    }
  ]);

  return (
    <div className="App">
        <RouterProvider router={router} />
    </div>
  );
}

