import React, { Component } from 'react';
import { NavMenu } from './NavMenu';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <NavMenu />
        <div className="content">
          {this.props.children}
        </div>
      </div>
    );
  }
}
