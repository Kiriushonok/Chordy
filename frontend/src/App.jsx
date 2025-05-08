import { BrowserRouter, Routes, Route } from "react-router-dom"
import './App.css'
import Header from './components/Header/Header'
import Footer from './components/Footer/Footer'
import Home from './pages/home/Home'
import Artists from './pages/artists/Artists'
import Collection from "./pages/collection/Collection"
import Popular from "./pages/popular/Popular"
import AddSong from "./pages/addSong/AddSong"
import Feedback from "./pages/feedback/Feedback"
import Rules from "./pages/rules/Rules"
import Login from "./pages/login/Login"

function App() {

  return (
    <BrowserRouter>
      <Header />
      <main className="app-content">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/artists" element={<Artists />} />
          <Route path="/collections/:collectionId" element={<Collection />} />
          <Route path="/popular" element={<Popular />} />
          <Route path="/add-song" element={<AddSong/>} />
          <Route path="/feedback" element={<Feedback />} />
          <Route path="/rules" element={<Rules />} />
          <Route path="/login" element={<Login />} />
        </Routes>
      </main>
      <Footer />
    </BrowserRouter>
  )
}

export default App
