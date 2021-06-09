import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    return (
      <div className={(this.state.collapsed)?"topnav":"topnav responsive"} id="myTopnav"> 
        <Link to="/">Home</Link>
        <Link to="/counter">counter</Link>
        <Link to="/fetch-data">Fetch data</Link>
        <Link to="/user-info">User info</Link>        
        <a href="javascript:void(0);" className="icon" onClick={this.toggleNavbar}>{this.state.collapsed?'+':'-'}</a>
      </div>     
    );
  }
}
