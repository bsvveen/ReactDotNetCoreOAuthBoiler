import React, { Component } from 'react';

export class UserInfo extends Component {
  static displayName = UserInfo.name;

  constructor (props) {
    super(props);
    this.state = { user: undefined, loading: true };

    fetch('Account/GetUserInfo')
      .then(response => response.json())
      .then(data => {
        this.setState({ user: data, loading: false });
      });
  }

  static renderUser(user) {
    return (<div>
      <div>Name: {user.name}</div>
      <div>Email: {user.email}</div>
      <div><img width="150px" src={user.image} alt="{user.Name}" /></div>
    </div>);
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : UserInfo.renderUser(this.state.user);

    return (
      <div>
        <h1>UserInfo</h1>        
        {contents}
      </div>
    );
  }
}
