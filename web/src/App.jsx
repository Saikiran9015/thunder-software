import React, { useState } from 'react';
import './index.css';

function App() {
  return (
    <div className="app">
      {/* Navigation */}
      <nav style={{ position: 'fixed', top: 0, width: '100%', padding: '1.5rem 0', zIndex: 100, backdropFilter: 'blur(10px)' }}>
        <div className="container" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ fontSize: '1.5rem', fontWeight: 'bold' }}>
            THUNDER<span className="text-gradient">CUT</span>
          </div>
          <div style={{ display: 'flex', gap: '2rem' }}>
            <a href="#features">Features</a>
            <a href="#download">Download</a>
            <a href="#contact">Contact</a>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <header className="hero">
        <div className="hero-bg-glow" />
        <div className="container">
          <span style={{ color: '#00e5ff', fontWeight: '600', letterSpacing: '2px', textTransform: 'uppercase', marginBottom: '1rem', display: 'block' }}>
            Version 2.0 Available Now
          </span>
          <h1>
            Precision Cutting for<br />
            <span className="text-gradient">Mobile Skins & Protection</span>
          </h1>
          <p>
            The ultimate software for managing, designing, and cutting screen protectors and mobile skins.
            Compatible with 5000+ device models.
          </p>
          <div className="hero-buttons">
            <a href="/ThunderCut-Setup.zip" className="btn btn-primary" download>Download v2.0</a>
            <a href="#demo" className="btn btn-outline">Watch Demo</a>
          </div>
        </div>
      </header>

      {/* Preview Section */}
      <section className="preview" id="demo">
        <div className="container">
          <div className="section-header">
            <h2>Designed for Professionals</h2>
            <p className="text-muted">Intuitive interface with powerful path editing capabilities.</p>
          </div>
          <div className="preview-mockup">
            {/* Placeholder for the app screenshot - using CSS to mimic a UI for now */}
            <div className="preview-content">Desktop Application Interface Preview</div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="features" id="features">
        <div className="container">
          <div className="section-header">
            <h2>Why Choose ThunderCut?</h2>
            <p>Built for speed, accuracy, and business growth.</p>
          </div>

          <div className="features-grid">
            <div className="feature-card">
              <div className="feature-icon">✂️</div>
              <h3>Precision Cutting</h3>
              <p>Advanced path algorithms ensure every cut is smooth and perfectly aligned with the device dimensions.</p>
            </div>
            <div className="feature-card">
              <div className="feature-icon">📱</div>
              <h3>Huge Database</h3>
              <p>Access templates for over 5,000 devices including phones, watches, cameras, and laptops. Updates daily.</p>
            </div>
            <div className="feature-card">
              <div className="feature-icon">🎨</div>
              <h3>Custom Design</h3>
              <p>Import your own vectors or use our built-in design tools to create custom skins and protectors.</p>
            </div>
          </div>
        </div>
      </section>

      {/* Call to Action */}
      <section className="hero" id="download" style={{ minHeight: '60vh' }}>
        <div className="container">
          <h2>Ready to Upgrade Your Shop?</h2>
          <p style={{ margin: '2rem auto' }}>Join thousands of businesses using ThunderCut to deliver premium protection.</p>
          <a href="#" className="btn btn-primary">Get Started for Free</a>
        </div>
      </section>

      <footer>
        <div className="container">
          <p>&copy; 2026 ThunderCut Software. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
}

export default App;
