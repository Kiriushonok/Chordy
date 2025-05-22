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
import Author from "./components/Author/Author"
import SearchResults from "./pages/SearchResults/SearchResults"
import Profile from "./pages/profile/Profile";
import ProtectedRoute from "./components/ProtectedRoute";
import Register from "./pages/register/Register";
import Song from "./pages/Song/Song";
import { useEffect } from 'react';
import AdminPanel from "./pages/admin/AdminPanel";

function App() {
  useEffect(() => {
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = 'https://fonts.googleapis.com/css2?family=Fira+Mono:wght@400;500&family=JetBrains+Mono:wght@400;700&family=Roboto+Mono:wght@400;700&family=PT+Mono&family=Source+Code+Pro:wght@400;700&display=swap';
    document.head.appendChild(link);
    return () => {
      document.head.removeChild(link);
    };
  }, []);

  return (
    <BrowserRouter>
      <Header />
      <main className="app-content">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/artists" element={<Artists />} />
          <Route path="/collections/:collectionId" element={<Collection />} />
          <Route path="/popular" element={<Popular />} />
          <Route path="/add-song" element={
            <ProtectedRoute>
              <AddSong />
            </ProtectedRoute>
          } />
          <Route path="/songs/:id" element={<Song />} />
          <Route path="/feedback" element={<Feedback />} />
          <Route path="/rules" element={<Rules />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/authors/:authorId" element={<Author />} />
          <Route path="/search" element={<SearchResults />} />
          <Route path="/profile" element={
            <ProtectedRoute>
              <Profile />
            </ProtectedRoute>
          } />
          <Route path="/admin" element={
            <ProtectedRoute>
              <AdminPanel />
            </ProtectedRoute>
          } />
        </Routes>
      </main>
      <Footer />
    </BrowserRouter>
  )
}

export default App
