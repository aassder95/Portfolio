const { DataTypes } = require("sequelize");
const sequelize = require("../config/database");

const Stage = sequelize.define(
  "Stage",
  {
    idx: {
      type: DataTypes.INTEGER,
      autoIncrement: true,
      primaryKey: true,
      allowNull: false,
    },
    user_uuid: {
      type: DataTypes.STRING(36),
      unique: true,
      allowNull: false,
    },
    single_clear_stage: {
      type: DataTypes.INTEGER,
      defaultValue: 0,
      allowNull: false,
    },
    single_clear_date: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      allowNull: false,
    },
    infinite_clear_wave: {
      type: DataTypes.INTEGER,
      defaultValue: 0,
      allowNull: false,
    },
    infinite_clear_date: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      allowNull: false,
    },
  },
  {
    tableName: "stage",
    timestamps: false,
  }
);

module.exports = Stage;
