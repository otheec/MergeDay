import {AppContextProvider} from "./context/AppContext.tsx";
import {ScrollToTop} from "./components/common/ScrollToTop.tsx";
import {BrowserRouter as Router, Outlet, Route, Routes} from "react-router";
import {AuthenticatedRoute} from "./routing/ProtectedRoute.tsx";
import AppLayout from "./layout/AppLayout.tsx";
import Home from "./pages/Dashboard/Home.tsx";
import Workspaces from "./pages/Workspaces.tsx";
import UserProfiles from "./pages/UserProfiles.tsx";
import Calendar from "./pages/Calendar.tsx";
import Blank from "./pages/Blank.tsx";
import FormElements from "./pages/Forms/FormElements.tsx";
import BasicTables from "./pages/Tables/BasicTables.tsx";
import Alerts from "./pages/UiElements/Alerts.tsx";
import Avatars from "./pages/UiElements/Avatars.tsx";
import Badges from "./pages/UiElements/Badges.tsx";
import Buttons from "./pages/UiElements/Buttons.tsx";
import Images from "./pages/UiElements/Images.tsx";
import Videos from "./pages/UiElements/Videos.tsx";
import LineChart from "./pages/Charts/LineChart.tsx";
import BarChart from "./pages/Charts/BarChart.tsx";
import SignIn from "./pages/AuthPages/SignIn.tsx";
import SignUp from "./pages/AuthPages/SignUp.tsx";
import NotFound from "./pages/OtherPage/NotFound.tsx";

export default function App() {
  return (
    <Router>
      <ScrollToTop/>
      <Routes>
        <Route element={<AppContextProvider><Outlet/></AppContextProvider>}>
          {/* Dashboard Layout */}
          <Route element={<AuthenticatedRoute/>}>
            <Route element={<AppLayout/>}>
              <Route index path="/" element={<Home/>}/>
              <Route path="/workspaces" element={<Workspaces/>}/>

              {/* Others Page */}
              <Route path="/profile" element={<UserProfiles/>}/>
              <Route path="/calendar" element={<Calendar/>}/>
              <Route path="/blank" element={<Blank/>}/>

              {/* Forms */}
              <Route path="/form-elements" element={<FormElements/>}/>

              {/* Tables */}
              <Route path="/basic-tables" element={<BasicTables/>}/>

              {/* Ui Elements */}
              <Route path="/alerts" element={<Alerts/>}/>
              <Route path="/avatars" element={<Avatars/>}/>
              <Route path="/badge" element={<Badges/>}/>
              <Route path="/buttons" element={<Buttons/>}/>
              <Route path="/images" element={<Images/>}/>
              <Route path="/videos" element={<Videos/>}/>

              {/* Charts */}
              <Route path="/line-chart" element={<LineChart/>}/>
              <Route path="/bar-chart" element={<BarChart/>}/>
            </Route>
          </Route>
          {/* Auth Layout */}
          <Route path="/signin" element={<SignIn/>}/>
          <Route path="/signup" element={<SignUp/>}/>

          {/* Fallback Route */}
          <Route path="*" element={<NotFound/>}/>
        </Route>
      </Routes>
    </Router>
  );
}
